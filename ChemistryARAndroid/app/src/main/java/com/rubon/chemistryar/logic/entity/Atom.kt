package com.rubon.chemistryar.logic.entity

data class Atom(
    val type: AtomType,
    val subdata: String
){
    override fun toString(): String {
        return type.name+"-"+subdata
    }
}

enum class AtomType(val valency: Int){
    H(1),
    O(-2),
    S(6),
    Cl(-1);

    companion object{
        fun fromName(name: String): AtomType {
            for (type in values())
                if (type.name == name)
                    return type
            throw Exception("Unknown atom name")
        }
    }
}