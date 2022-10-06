package com.rubon.chemistryar.screen.unity

import com.rubon.chemistryar.logic.entity.Atom
import com.rubon.chemistryar.logic.entity.AtomType
import com.rubon.chemistryar.logic.use_case.ChemistryManager
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class MainViewModel @Inject constructor(){
    fun getMolecules(input: String): String{
        val connections = parseInput(input)

        val molecules = ArrayList<List<Atom>>()
        for (connection in connections){
            val res = ChemistryManager.checkMolecule(connection)
            if (res.isNotEmpty())
                molecules += res
        }

        return joinOutput(molecules)
    }

    private fun parseInput(input: String): List<List<Atom>>{
        val connections = input.split(";")

        val result = ArrayList<List<Atom>>()
        for (line in connections){
            val atoms = line.split(",")
            val connection = ArrayList<Atom>()
            for (atom in atoms) {
                val parts = atom.split("-")
                connection += Atom(AtomType.fromName(parts[0]), parts[1])
            }
            result += connection
        }

        return result
    }

    private fun joinOutput(molecules: List<List<Atom>>): String{
        val result = StringBuilder()
        for (molecule in molecules){
            for (atom in molecule)
                result.append(atom.toString()).append(",")
            result.deleteCharAt(result.lastIndex)
            result.append(";")
        }
        if (result.isNotEmpty())
            result.deleteCharAt(result.lastIndex)

        return result.toString()
    }
}