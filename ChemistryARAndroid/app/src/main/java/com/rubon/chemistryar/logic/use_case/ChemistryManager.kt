package com.rubon.chemistryar.logic.use_case

import com.rubon.chemistryar.logic.entity.Atom

object ChemistryManager {
    fun checkMolecule(atoms: List<Atom>):List<Atom>{
        val maxMask = 2.pow(atoms.size)-1
        var bestMask = 0

        for (mask in 1..maxMask)
            if (testByMask(atoms, mask))
                if (mask1Count(bestMask)< mask1Count(mask))
                    bestMask = mask

        val result = ArrayList<Atom>()
        for (i in atoms.indices)
            if (2.pow(i) and bestMask > 0)
                result += atoms[i]

        return result
    }

    private fun testByMask(atoms: List<Atom>, mask: Int): Boolean{
        var result = 0
        for (i in atoms.indices)
            if (2.pow(i) and mask > 0)
                result += atoms[i].type.valency

        return result == 0
    }

    private fun mask1Count(mask: Int): Int{
        var res = 0
        var x = mask

        while (x>0){
            res += x%2
            x /=2
        }

        return res
    }
}

private fun Int.pow(_st: Int): Int {
    if (_st==0) return 1
    var x = this
    var z = 1
    var st = _st

    while (st>1){
        if (st%2==0){
            x *= x
            st /= 2
        } else {
            z *= x
            st--
        }
    }

    return x*z
}
